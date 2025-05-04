import {
  PurchaseCreateDto,
  TicketPurchaseRequest,
} from './../../services/purchase/purchase.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  TicketDto,
  TicketService,
  TicketStatus,
  TicketType,
} from '../../services/ticket/ticket.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PurchaseService } from '../../services/purchase/purchase.service';
import { jwtDecode } from 'jwt-decode';
import { UserService } from '../../services/user/user.service';
import { PromoCodeService } from '../../services/promo-code/promo-code.service';
import Swal from 'sweetalert2';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-ticket-modal',
  imports: [CommonModule, FormsModule],
  templateUrl: './ticket-modal.component.html',
  styleUrl: './ticket-modal.component.css',
})
export class TicketModalComponent implements OnInit {
  eventId: number = 0;
  ticketId: number = 0;
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
  ticketPurchaseRequest: TicketPurchaseRequest = {
    ticketId: 0,
    quantity: 0,
  };
  purchaseCreateDto: PurchaseCreateDto = {
    tickets: [],
    userId: 0,
  };

  userId: number = 0;

  purchaseQuantity: number = 1;
  promoCode: string = '';
  promoErrorMessage: string = '';
  promoSuccessMessage: string = '';
  saleAmountInPercentage: number = 0;

  isPurchasing: boolean = false;

  constructor(
    private userService: UserService,
    private ticketService: TicketService,
    private purchaseService: PurchaseService,
    private promoCodeService: PromoCodeService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((params) => {
      this.eventId = +params.get('eventId')!;
    });

    this.route.paramMap.subscribe((params) => {
      const param = params.get('ticketId');
      if (param) {
        this.ticketId = +param;
        this.getTicket();
        this.getUserInfo();
      }
    });
  }

  getTicket() {
    this.ticketService.getTicketById(this.ticketId).subscribe({
      next: (data: any) => {
        this.ticket = data.ticket;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Ticket fetched successfully!');
      },
    });
  }

  purchaseTicket() {
    this.isPurchasing = true;

    this.ticketPurchaseRequest.quantity = this.purchaseQuantity;
    this.ticketPurchaseRequest.ticketId = this.ticketId;

    this.purchaseCreateDto.tickets.push(this.ticketPurchaseRequest);
    this.purchaseCreateDto.userId = this.userId;
    if (this.promoCode != '') {
      this.purchaseCreateDto.promoCodeText = this.promoCode;
    } else {
      this.purchaseCreateDto.promoCodeText = undefined;
    }

    let message = '';

    this.purchaseService.purchaseTicket(this.purchaseCreateDto).subscribe({
      next: (data: any) => {
        message = data.purchaseDto;
      },
      error: (err: HttpErrorResponse) => {
        if (err.status === 403) {
          Swal.fire(
            'Oops!',
            'You do not have a permission to make this purchase!',
            'error'
          );
          this.isPurchasing = false;
        }

        if (err.error.Message) {
          message = err.error.Message;
          Swal.fire('Oops!', message, 'error');
        } else if (err.error.errors) {
          message = err.error.errors.PromoCodeText[0];
          Swal.fire('Oops!', message, 'error');
        }
      },
      complete: () => {
        this.isPurchasing = false;
        Swal.fire('Success!', message, 'success');
        console.log('Ticket purchased successfully!');
        this.router.navigate(['/events/event', this.eventId]);
      },
    });
  }

  getPromoCode() {
    if (!this.promoCode?.trim()) {
      this.promoErrorMessage = 'Please enter a promo code.';
      this.saleAmountInPercentage = 0;
      return;
    }

    this.promoCodeService.getPromoCodeBySearchTerm(this.promoCode).subscribe({
      next: (data: any) => {
        if (data?.promoCode) {
          this.saleAmountInPercentage = data.promoCode.saleAmountInPercentages;
          this.promoSuccessMessage = 'Promo code applied!';
          this.promoErrorMessage = '';
        } else {
          this.saleAmountInPercentage = 0;
          this.promoErrorMessage = 'Invalid promo code.';
          this.promoSuccessMessage = '';
        }
      },
      error: (err) => {
        console.error(err);
        this.saleAmountInPercentage = 0;
        this.promoErrorMessage = 'Invalid promo code.';
      },
      complete: () => {
        console.log('Promo Code Applied successfully!');
      },
    });
  }

  calculatePrice(ticketPrice: number) {
    return (
      this.purchaseQuantity *
      (ticketPrice - (ticketPrice * this.saleAmountInPercentage) / 100)
    );
  }

  closeModal() {
    this.router.navigate([{ outlets: { modal: null } }], {
      relativeTo: this.route.parent,
    });
  }

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.userId = decoded.nameid;
  }

  getTicketType(type: number) {
    return TicketType[type];
  }
}
