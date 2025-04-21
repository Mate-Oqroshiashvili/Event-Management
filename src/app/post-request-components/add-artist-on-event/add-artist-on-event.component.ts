import { Component, OnInit } from '@angular/core';
import {
  UserDto,
  UserService,
  UserType,
} from '../../services/user/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import {
  EventCategory,
  EventDto,
  EventService,
  EventStatus,
} from '../../services/event/event.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-add-artist-on-event',
  imports: [],
  templateUrl: './add-artist-on-event.component.html',
  styleUrl: './add-artist-on-event.component.css',
})
export class AddArtistOnEventComponent implements OnInit {
  eventId: number = 0;
  event: EventDto = {
    id: 0,
    title: '',
    description: '',
    startDate: null,
    endDate: null,
    capacity: 0,
    status: EventStatus.DRAFT,
    category: EventCategory.No_Category,
    bookedStaff: 0,
    images: [],
    location: null,
    organizer: null,
    tickets: [],
    speakersAndArtists: [],
    reviews: [],
    comments: [],
  };
  artists: UserDto[] = [];

  constructor(
    private userService: UserService,
    private eventService: EventService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('eventId');
      if (id) {
        this.eventId = +id;
        this.getArtists();
        this.getEvent();
      }
    });
  }

  getArtists() {
    this.userService.getArtists().subscribe({
      next: (data: any) => {
        this.artists = data.artists;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Artists fetched successfully!');
      },
    });
  }

  addArtist(userId: number) {
    let message = '';

    this.eventService
      .addSpeakerOrArtistOnEvent(this.eventId, userId)
      .subscribe({
        next: (data: any) => {
          message = `Artist ${data.userDto.name} successfully added on the event!`;
        },
        error: (err) => {
          message = err.error.Message;
          Swal.fire('Oops!', message, 'error');
          console.error(err);
        },
        complete: () => {
          this.getEvent();
          Swal.fire('Success!', message, 'success');
        },
      });
  }

  removeArtist(userId: number) {
    let message = '';

    this.eventService
      .removeSpeakerOrArtistFromEvent(this.eventId, userId)
      .subscribe({
        next: (data: any) => {
          message = data.result;
          console.log(data);
        },
        error: (err) => {
          message = err.error.Message;
          Swal.fire('Oops!', message, 'error');
          console.error(err);
        },
        complete: () => {
          this.getEvent();
          Swal.fire('Success!', message, 'success');
        },
      });
  }

  getEvent() {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (data: any) => {
        this.event = data.event;
      },
      error: (err) => {
        console.error(err);
      },
    });
  }

  isAdded(userId: number) {
    return this.event.speakersAndArtists.find(
      (x) => x.id === userId && x.userType === UserType.ARTIST
    );
  }

  closeModal() {
    this.router.navigate([{ outlets: { 'add-modal': null } }], {
      relativeTo: this.route.parent,
    });
  }
}
