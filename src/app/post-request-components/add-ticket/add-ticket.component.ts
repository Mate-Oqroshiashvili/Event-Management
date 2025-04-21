import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  TicketCreateDto,
  TicketService,
} from '../../services/ticket/ticket.service';
import { ActivatedRoute, Router } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-add-ticket',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-ticket.component.html',
  styleUrl: './add-ticket.component.css',
})
export class AddTicketComponent implements OnInit {
  organizerId: number = 0;
  eventId: number = 0;
  ticketForm!: FormGroup;
  backendErrors: { [key: string]: string[] } = {};

  constructor(
    private fb: FormBuilder,
    private ticketService: TicketService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.eventId = +params.get('eventId')!;
      this.organizerId = +params.get('organizerId')!;
    });

    this.ticketForm = this.fb.group({
      type: [0, Validators.required],
      price: [
        null,
        [Validators.required, Validators.min(1), Validators.max(10000)],
      ],
      quantity: [null, [Validators.required, Validators.min(1)]],
    });
  }

  addTicket(): void {
    const dto: TicketCreateDto = this.ticketForm.value;
    dto.eventId = this.eventId;

    let message = '';

    this.ticketService.addTicket(dto).subscribe({
      next: (data: any) => {
        message = 'Ticket added successfully!';
        this.router.navigate(['/organizer-panel', this.organizerId]);
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors) {
          this.backendErrors = err.error.errors;
        }
        if (err.error.Message) {
          message = err.error.Message;
          Swal.fire('Oops!', message, 'error');
        }
        console.error(err);
      },
      complete: () => {
        Swal.fire('Success!', message, 'success');
      },
    });
  }

  getFieldErrors(field: string): string | null {
    if (this.backendErrors[field]) {
      const filteredErrors = this.backendErrors[field].filter(
        (msg) => !msg.toLowerCase().includes('field')
      );
      return filteredErrors.join('<br>') || null;
    }
    return null;
  }
}
