import {
  CommentCreateDto,
  CommentDto,
  CommentUpdateDto,
} from './../../../services/comment/comment.service';
import { CommonModule } from '@angular/common';
import {
  EventCategory,
  EventDto,
  EventService,
  EventStatus,
} from '../../../services/event/event.service';
import { Component, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterModule,
} from '@angular/router';
import {
  TicketDto,
  TicketService,
  TicketType,
} from '../../../services/ticket/ticket.service';
import {
  Role,
  UserDto,
  UserService,
  UserType,
} from '../../../services/user/user.service';
import { jwtDecode } from 'jwt-decode';
import { CommentService } from '../../../services/comment/comment.service';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  ReviewCreateDto,
  ReviewDto,
  ReviewService,
} from '../../../services/review/review.service';
import { filter } from 'rxjs';
import Swal from 'sweetalert2';
import { CommentSocketService } from '../../../services/web sockets/comment-socket.service';
import { ReviewSocketService } from '../../../services/web sockets/review-socket.service';

@Component({
  selector: 'app-event-page',
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './event-page.component.html',
  styleUrls: ['./event-page.component.css', './responsive.css'],
})
export class EventPageComponent implements OnInit {
  loggedInUserRole: string = '';
  eventId: number = 0;
  userId: number = 0;
  loggedInUserId: number = 0;
  event: EventDto = {
    id: 0,
    title: '',
    description: '',
    startDate: null,
    endDate: null,
    capacity: 0,
    status: EventStatus.DRAFT,
    bookedStaff: 0,
    images: [],
    location: null,
    organizer: null,
    tickets: [],
    speakersAndArtists: [],
    reviews: [],
    comments: [],
    category: EventCategory.IT_And_Technologies,
  };
  tickets: TicketDto[] = [];
  reviewsResult: number = 0;

  addCommentForm!: FormGroup;
  editCommentForm!: FormGroup;

  commentId: number = 0;
  editing: boolean = false;

  reviewDto: ReviewCreateDto = {
    starCount: 0,
    userId: 0,
    eventId: 0,
  };
  retrievedReview: ReviewDto = {
    id: 0,
    starCount: 0,
    userId: 0,
    user: undefined,
    eventId: 0,
  };
  hasReviewed: boolean = false;

  stars = Array(5).fill(0);
  selectedRating: number = 0;
  hoverRating: number = 0;

  modalActive = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private eventService: EventService,
    private ticketService: TicketService,
    private commentService: CommentService,
    private reviewService: ReviewService,
    private commentSocket: CommentSocketService,
    private reviewSocket: ReviewSocketService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const eventIdParam = data.get('eventId');
      if (eventIdParam) {
        this.eventId = +eventIdParam;
        this.getUserInfo();
        this.getEventById();
        this.getTicketsByEventId();
        this.getReviewByUserId();

        this.addCommentForm = this.fb.group({
          commentContent: ['', Validators.required],
        });

        this.editCommentForm = this.fb.group({
          commentContent: ['', Validators.required],
        });
      } else {
        console.error('Event ID not found in route parameters');
      }
    });

    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        this.modalActive = !!this.route.children.find(
          (r) => r.outlet === 'modal'
        );
      });

    this.commentSocket.startConnection();
    this.reviewSocket.startConnection();

    this.commentSocketMethods();
    this.reviewSocketMethods();
  }

  commentSocketMethods() {
    this.commentSocket.comment$.subscribe((data) => {
      if (data?.commentDto && data?.commentDto.eventId === this.eventId) {
        // Add the comment from this.event.comments
        let comment: CommentDto = data?.commentDto;
        let user: UserDto = {
          id: data?.userObject?.id,
          name: data?.userObject?.name,
          email: '',
          phoneNumber: '',
          profilePicture: data?.userObject?.profilePicture,
          role: data?.userObject?.role,
          userType: data?.userObject?.userType,
          balance: 0,
          codeExpiration: undefined,
          isLoggedIn: false,
          organizer: null,
          tickets: [],
          purchases: [],
          participants: [],
          reviews: [],
          comments: [],
          usedPromoCodes: [],
        };
        comment.user = user;
        this.event.comments.push(comment);
      }
    });

    this.commentSocket.deletedComment$.subscribe((commentId) => {
      if (commentId != null) {
        // Remove the comment from this.event.comments
        this.event.comments = this.event.comments.filter(
          (c) => c.id !== commentId
        );
      }
    });

    // Update the comment in this.event.comments
    this.commentSocket.updatedComment$.subscribe((data) => {
      if (data?.id && data?.commentContent && data?.eventId === this.eventId) {
        // Update the comment in the UI
        const index = this.event.comments.findIndex((c) => c.id === data.id);
        if (index !== -1) {
          this.event.comments[index].commentContent = data.commentContent;
        }
      }
    });
  }

  reviewSocketMethods() {
    this.reviewSocket.review$.subscribe((data) => {
      if (data?.reviewDto && data?.reviewDto.eventId === this.eventId) {
        // Add the review from this.event.reviews
        let review: ReviewDto = data?.reviewDto;
        let user: UserDto = {
          id: data?.userObject?.id,
          name: data?.userObject?.name,
          email: '',
          phoneNumber: '',
          profilePicture: data?.userObject?.profilePicture,
          role: data?.userObject?.role,
          userType: data?.userObject?.userType,
          balance: 0,
          codeExpiration: undefined,
          isLoggedIn: false,
          organizer: null,
          tickets: [],
          purchases: [],
          participants: [],
          reviews: [],
          comments: [],
          usedPromoCodes: [],
        };
        review.user = user;

        const alreadyExists = this.event.reviews.some(
          (r) => r.userId === review.userId
        );

        if (!alreadyExists) {
          this.event.reviews.push(review);

          const totalStars = this.event.reviews.reduce(
            (sum: number, review: any) => sum + review.starCount,
            0
          );
          this.reviewsResult = totalStars / this.event.reviews.length;
        }
      }
    });

    this.reviewSocket.deletedReview$.subscribe((reviewId) => {
      if (reviewId != null) {
        // Remove the review from this.event.reviews
        this.event.reviews = this.event.reviews.filter(
          (c) => c.id !== reviewId
        );
      }
    });

    // Update the review in this.event.reviews
    this.reviewSocket.updatedReview$.subscribe((data) => {
      if (data?.id && data?.starCount && data?.eventId === this.eventId) {
        // Update the review in the UI
        const index = this.event.reviews.findIndex((c) => c.id === data.id);
        if (index !== -1) {
          this.event.reviews[index].starCount = data.starCount;
        }
      }
    });
  }

  getEventById() {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (data: any) => {
        this.event = data.event;
        const reviews = this.event.reviews;

        if (reviews && reviews.length > 0) {
          const totalStars = reviews.reduce(
            (sum: number, review: any) => sum + review.starCount,
            0
          );
          this.reviewsResult = totalStars / reviews.length;
        } else {
          this.reviewsResult = 0;
        }

        let review = reviews.find((x) => x.userId == this.userId);
        if (review != null) {
          this.hasReviewed = true;
        } else {
          this.hasReviewed = false;
        }
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Event fetched successfully!');
      },
    });
  }

  getTicketsByEventId() {
    this.ticketService.getTicketsByEventId(this.eventId).subscribe({
      next: (data: any) => {
        this.tickets = data.tickets;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Tickets fetched successfully!');
      },
    });
  }

  deleteEvent() {
    let message = '';

    this.eventService.removeEvent(this.eventId).subscribe({
      next: (data: any) => {
        message = data.message;
      },
      error: (err) => {
        message = err.error.Message;
        Swal.fire('Oops!', message, 'error');
        console.error(err);
      },
      complete: () => {
        Swal.fire('Success', message, 'success');
        this.router.navigate(['/events']);
      },
    });
  }

  addComment() {
    if (this.addCommentForm.invalid) return;

    const commentContent = this.addCommentForm.value.commentContent;

    const comment: CommentCreateDto = {
      commentContent: commentContent,
      userId: this.userId,
      eventId: this.eventId,
    };

    this.commentService.addComment(comment).subscribe({
      next: (data: any) => {
        this.addCommentForm.reset();
      },
      error: (err) => {
        console.error(err);
      },
    });
  }

  editActions(id: number) {
    this.commentService.getCommentById(id).subscribe({
      next: (data: any) => {
        this.commentId = data.comment.id;
        this.editCommentForm.patchValue({
          commentContent: data.comment.commentContent,
        });
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.editing = true;
      },
    });
  }

  editComment() {
    if (this.editCommentForm.invalid) return;

    const updatedContent = this.editCommentForm.value.commentContent;

    const updateDto: CommentUpdateDto = {
      commentContent: updatedContent,
    };

    this.commentService
      .updateComment(this.commentId, this.userId, updateDto)
      .subscribe({
        next: (data: any) => {
          this.editing = false;
          this.getEventById();
          this.editCommentForm.reset();
        },
        error: (err) => {
          console.error(err);
        },
      });
  }

  cancel() {
    this.editing = false;
  }

  deleteComment(commentId: number, userId: number) {
    let message = '';

    this.commentService.removeComment(commentId, userId).subscribe({
      next: (data: any) => {
        message = data.message;
      },
      error: (err) => {
        message = err.error.Message;
        Swal.fire('Oops!', message, 'error');
        console.error(err);
      },
      complete: () => {
        Swal.fire('Success!', message, 'success');
      },
    });
  }

  addReview() {
    this.reviewDto.userId = this.userId;
    this.reviewDto.eventId = this.eventId;
    this.reviewDto.starCount = this.selectedRating;

    let message = '';
    let messages: string[] = [];

    this.reviewService.addReview(this.reviewDto).subscribe({
      next: (data: any) => {
        let review = this.event.reviews.find((x) => x.userId == this.userId);
        if (review != null) {
          this.hasReviewed = true;
        } else {
          this.hasReviewed = false;
        }

        this.getReviewByUserId();
        this.getEventById();
        Swal.fire('Success!', 'Review Added Successfully!', 'success');
      },
      error: (err) => {
        if (err.error.Message) {
          message = err.error.Message;
          Swal.fire('Oops!', message, 'error');
        } else if (err.error.errors) {
          err.error.errors.StarCount.forEach((element: any) => {
            messages.push(element);
          });
          message = messages.join('<br>');
          Swal.fire('Oops!', message, 'error');
        } else {
          Swal.fire('Oops!', 'Something went wrong!', 'error');
        }
        console.error(err);
      },
    });
  }

  getReviewByUserId() {
    this.reviewService.getReviewsByUserId(this.userId).subscribe({
      next: (data: any) => {
        this.retrievedReview = data.reviews.filter(
          (x: ReviewDto) => x.eventId == this.eventId
        )[0];
        if (!this.retrievedReview) {
          this.hasReviewed = false;
        } else {
          this.hasReviewed = true;
        }
      },
      error: (err) => {
        console.error(err);
      },
    });
  }

  deleteReview() {
    this.reviewService
      .removeReview(this.retrievedReview.id, this.userId)
      .subscribe({
        next: (data: any) => {
          this.hasReviewed = false;
          this.getReviewByUserId();
          this.getEventById();
          Swal.fire('Success', data.message, 'success');
        },
        error: (err) => {
          console.error(err);
          if (err.error.Message) {
            Swal.fire('Oops!', err.error.Message, 'error');
          }
        },
      });
  }

  deleteTicket(ticketId: number) {
    let message = '';

    this.ticketService.removeTicket(ticketId).subscribe({
      next: (data: any) => {
        message = data.message;
        Swal.fire('Success!', message, 'success');
      },
      error: (err) => {
        if (err.error.Message) {
          message = err.error.Message;
          Swal.fire('Oops!', message, 'error');
        }
        console.error(err);
      },
      complete: () => {
        this.getEventById();
        this.getTicketsByEventId();
      },
    });
  }

  onDeleteTicketClick(event: MouseEvent, ticketId: number): void {
    event.stopPropagation();
    event.preventDefault();

    this.deleteTicket(ticketId);
  }

  getEventStatus(status: number): string {
    return EventStatus[status] ?? 'Unknown Status';
  }

  getTicketStatus(status: number): string {
    return TicketType[status] ?? 'Unknown Status';
  }

  getUserType(type: number): string {
    return UserType[type] ?? 'Unknown Type';
  }

  getCategory(category: number): string {
    let categoryText = EventCategory[category] ?? 'Unknown Status';
    let result = categoryText.replaceAll('_', ' ');
    return result;
  }

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.userId = decoded.nameid;
    this.loggedInUserRole = decoded.role;
  }

  setRating(rating: number): void {
    this.selectedRating = rating;
  }
}
