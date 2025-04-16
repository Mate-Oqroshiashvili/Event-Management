import { Component, OnInit } from '@angular/core';
import { UserDto, UserService } from '../../services/user/user.service';
import { ActivatedRoute } from '@angular/router';
import { EventService } from '../../services/event/event.service';

@Component({
  selector: 'app-add-artist-on-event',
  imports: [],
  templateUrl: './add-artist-on-event.component.html',
  styleUrl: './add-artist-on-event.component.css',
})
export class AddArtistOnEventComponent implements OnInit {
  eventId: number = 0;
  artists: UserDto[] = [];

  constructor(
    private userService: UserService,
    private eventService: EventService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.eventId = +params.get('eventId')!;
    });

    this.getArtists();
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
    this.eventService
      .addSpeakerOrArtistOnEvent(this.eventId, userId)
      .subscribe({
        next: (data: any) => {
          console.log(data);
        },
        error: (err) => {
          console.error(err);
        },
        complete: () => {
          alert('Artist Added Successfully!');
        },
      });
  }
}
