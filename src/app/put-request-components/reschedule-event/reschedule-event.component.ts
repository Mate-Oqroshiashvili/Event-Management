import { Component, OnInit } from '@angular/core';
import {
  EventService,
  RescheduleEventDto,
} from '../../services/event/event.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-reschedule-event',
  imports: [FormsModule],
  templateUrl: './reschedule-event.component.html',
  styleUrl: './reschedule-event.component.css',
})
export class RescheduleEventComponent implements OnInit {
  eventId: number = 0;
  organizerId: number = 0;
  newDate: string = this.formatDateForInput(new Date());
  errorMessage: string = '';

  constructor(
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const eventIdParam = data.get('eventId');
      const organizerIdParam = data.get('organizerId');
      if (eventIdParam && organizerIdParam) {
        this.eventId = +eventIdParam;
        this.organizerId = +organizerIdParam;
      } else {
        console.error('Event ID not found in route parameters');
      }
    });
  }

  rescheduleEvent() {
    const parsedDate = new Date(this.newDate);
    const utcDate = new Date(
      parsedDate.getTime() - parsedDate.getTimezoneOffset() * 60000
    );

    const dto: RescheduleEventDto = {
      newDate: utcDate,
    };

    console.log('Parsed Date:', dto.newDate);

    this.eventService.rescheduleEvent(this.eventId, dto).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        this.errorMessage = err.error.Message;
        console.error(err);
      },
      complete: () => {
        this.router.navigate(['/organizer-panel', this.organizerId]);
      },
    });
  }

  formatDateForInput(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }
}
