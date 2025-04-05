import {
  CommentCreateDto,
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
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import {
  TicketDto,
  TicketService,
  TicketType,
} from '../../../services/ticket/ticket.service';
import { UserService } from '../../../services/user/user.service';
import { jwtDecode } from 'jwt-decode';
import {
  CommentDto,
  CommentService,
} from '../../../services/comment/comment.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-event-page',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './event-page.component.html',
  styleUrl: './event-page.component.css',
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
  commentCreateDto: CommentCreateDto = {
    commentContent: '',
    userId: 0,
    eventId: 0,
  };
  reviewsResult: number = 0;
  commentUpdateDto: CommentUpdateDto = {
    commentContent: '',
  };
  commentId: number = 0;
  editing: boolean = false;

  constructor(
    private userService: UserService,
    private eventService: EventService,
    private ticketService: TicketService,
    private commentService: CommentService,
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
      } else {
        console.error('Event ID not found in route parameters');
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

        console.log(data);
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
        console.log(data);
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
    this.eventService.removeEvent(this.eventId).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.router.navigate(['/events']);
      },
    });
  }

  addComment() {
    this.commentCreateDto.eventId = this.eventId;
    this.commentCreateDto.userId = this.userId;

    this.commentService.addComment(this.commentCreateDto).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.getEventById();
      },
    });
  }

  editActions(id: number) {
    this.commentService.getCommentById(id).subscribe({
      next: (data: any) => {
        this.commentId = data.comment.id;
        this.commentUpdateDto.commentContent = data.comment.commentContent;
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
    this.commentService
      .updateComment(this.commentId, this.userId, this.commentUpdateDto)
      .subscribe({
        next: (data: any) => {
          console.log(data);
        },
        error: (err) => {
          console.error(err);
        },
        complete: () => {
          this.editing = false;
          this.getEventById();
        },
      });
  }

  cancel() {
    this.editing = false;
  }

  deleteComment(commentId: number, userId: number) {
    console.log(commentId);
    console.log(userId);

    this.commentService.removeComment(commentId, userId).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.getEventById();
      },
    });
  }

  getEventStatus(status: number): string {
    return EventStatus[status] ?? 'Unknown Status';
  }

  getTicketStatus(status: number): string {
    return TicketType[status] ?? 'Unknown Status';
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
}
