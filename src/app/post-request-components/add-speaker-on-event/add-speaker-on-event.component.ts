import { Component, OnInit } from '@angular/core';
import { UserDto, UserService } from '../../services/user/user.service';
import { EventService } from '../../services/event/event.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-add-speaker-on-event',
  imports: [],
  templateUrl: './add-speaker-on-event.component.html',
  styleUrl: './add-speaker-on-event.component.css',
})
export class AddSpeakerOnEventComponent implements OnInit {
  eventId: number = 0;
  speakers: UserDto[] = [];

  constructor(
    private userService: UserService,
    private eventService: EventService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.eventId = +params.get('eventId')!;
    });

    this.getSpeakers();
  }

  getSpeakers() {
    this.userService.getSpeakers().subscribe({
      next: (data: any) => {
        this.speakers = data.speakers;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Speakers fetched successfully!');
      },
    });
  }

  addSpeaker(userId: number) {
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
          alert('Speaker Added Successfully!');
        },
      });
  }
}
