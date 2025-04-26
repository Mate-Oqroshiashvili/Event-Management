import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import {
  ParticipantDto,
  ParticipantService,
} from '../../../services/participant/participant.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TicketStatus } from '../../../services/ticket/ticket.service';
import { EventStatus } from '../../../services/event/event.service';

@Component({
  selector: 'app-user-participation-history',
  imports: [CommonModule],
  templateUrl: './user-participation-history.component.html',
  styleUrl: './user-participation-history.component.css',
})
export class UserParticipationHistoryComponent implements OnInit {
  userId: number = 0;
  participants: ParticipantDto[] = [];
  refunding: boolean = false;

  constructor(
    private participantService: ParticipantService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((params) => {
      this.userId = +params.get('userId')!;
      this.getParticipants();
    });
  }

  getParticipants() {
    this.participantService.getParticipantsByUserId(this.userId).subscribe({
      next: (data: any) => {
        const filtered = data.participantDtos.filter(
          (x: ParticipantDto) => x.event.status == EventStatus.COMPLETED
        );
        this.participants = filtered;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Participants fetched successfully!');
      },
    });
  }

  refund(participantId: number, purchaseId: number) {
    this.refunding = true;
    let message = '';

    this.participantService
      .requestTheRefund(participantId, purchaseId)
      .subscribe({
        next: (data: any) => {
          message = data.message;
        },
        error: (err) => {
          this.refunding = false;
          console.error(err);
        },
        complete: () => {
          this.refunding = false;
          Swal.fire('Sucess!', message, 'success');
          this.getParticipants();
        },
      });
  }
}
