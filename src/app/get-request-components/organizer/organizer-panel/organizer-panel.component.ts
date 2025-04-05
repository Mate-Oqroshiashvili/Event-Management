import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { EventDto, EventService } from '../../../services/event/event.service';

@Component({
  selector: 'app-organizer-panel',
  imports: [CommonModule, RouterModule],
  templateUrl: './organizer-panel.component.html',
  styleUrl: './organizer-panel.component.css',
})
export class OrganizerPanelComponent implements OnInit {
  organizerId: number = 0;
  result: EventDto[] = [];

  constructor(
    private eventService: EventService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      this.organizerId = +data.get('organizerId')!;
      this.getDrafted();
    });
  }

  getDrafted() {
    this.eventService.getDraftedEvents().subscribe({
      next: (data: any) => {
        this.result = [];
        this.result = data.events.filter(
          (x: EventDto) => x.organizer?.id === this.organizerId
        );
        console.log(this.result);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Fetched drafted events successfully!');
      },
    });
  }

  getPublished() {
    this.eventService.getPublishedEvents().subscribe({
      next: (data: any) => {
        this.result = [];
        this.result = data.events.filter(
          (x: EventDto) => x.organizer?.id === this.organizerId
        );
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Fetched drafted events successfully!');
      },
    });
  }

  getCompleted() {
    this.eventService.getCompletedEvents().subscribe({
      next: (data: any) => {
        this.result = [];
        this.result = data.events.filter(
          (x: EventDto) => x.organizer?.id === this.organizerId
        );
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Fetched drafted events successfully!');
      },
    });
  }

  getRemoved() {
    this.eventService.getDeletedEvents().subscribe({
      next: (data: any) => {
        this.result = [];
        this.result = data.events.filter(
          (x: EventDto) => x.organizer?.id === this.organizerId
        );
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Fetched drafted events successfully!');
      },
    });
  }
}
