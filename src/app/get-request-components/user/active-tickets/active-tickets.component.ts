import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import Swal from 'sweetalert2';
import {
  ParticipantDto,
  ParticipantService,
} from '../../../services/participant/participant.service';
import { EventStatus } from '../../../services/event/event.service';

@Component({
  selector: 'app-active-tickets',
  imports: [CommonModule],
  templateUrl: './active-tickets.component.html',
  styleUrl: './active-tickets.component.css',
})
export class ActiveTicketsComponent implements OnInit {
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
          (x: ParticipantDto) => x.event.status == EventStatus.PUBLISHED
        );
        this.participants = filtered;
        console.log(data);
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
          console.log(data);
        },
        error: (err) => {
          this.refunding = false;
          console.error(err);
        },
        complete: () => {
          this.refunding = false;
          this.participantService.notifyRefundCompleted();
          Swal.fire('Sucess!', message, 'success');
          this.getParticipants();
        },
      });
  }
}
