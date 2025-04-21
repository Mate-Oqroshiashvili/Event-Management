import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import Swal from 'sweetalert2';
import {
  TicketService,
  TicketDto,
  TicketStatus,
  TicketType,
  TicketUpdateDto,
} from '../../services/ticket/ticket.service';

@Component({
  selector: 'app-update-ticket',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './update-ticket.component.html',
  styleUrl: './update-ticket.component.css',
})
export class UpdateTicketComponent implements OnInit {
  ticketId: number = 0;
  eventId: number = 0;
  ticket: TicketDto = {
    id: 0,
    type: TicketType.BASIC,
    price: 0,
    quantity: 0,
    status: TicketStatus.AVAILABLE,
    qrCodeData: '',
    qrCodeImageUrl: '',
    event: null,
    users: [],
    purchases: [],
    participants: [],
  };
  ticketUpdateForm!: FormGroup;
  backendErrors: { [key: string]: string[] } = {};

  constructor(
    private fb: FormBuilder,
    private ticketService: TicketService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.ticketId = +params.get('ticketId')!;
      this.eventId = +params.get('eventId')!;
      this.getTicket();
    });

    this.ticketUpdateForm = this.fb.group({
      type: [null, Validators.required],
      price: [
        null,
        [Validators.required, Validators.min(1), Validators.max(10000)],
      ],
      quantity: [null, [Validators.required, Validators.min(1)]],
    });
  }

  getTicket() {
    this.ticketService.getTicketById(this.ticketId).subscribe({
      next: (data: any) => {
        this.ticket = data.ticket;
        console.log(data);

        this.ticketUpdateForm.patchValue({
          type: this.ticket.type,
          price: this.ticket.price,
          quantity: this.ticket.quantity,
        });
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Ticket fetched successfully!');
      },
    });
  }

  updateTicket(): void {
    const dto: TicketUpdateDto = this.ticketUpdateForm.value;

    let message = '';

    this.ticketService.updateTicket(this.ticketId, dto).subscribe({
      next: (data: any) => {
        message = data.message;
        this.router.navigate(['/events/event', this.eventId]);
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
