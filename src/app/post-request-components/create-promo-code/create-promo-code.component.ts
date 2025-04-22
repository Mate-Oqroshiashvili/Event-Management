import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  PromoCodeCreateDto,
  PromoCodeService,
} from '../../services/promo-code/promo-code.service';
import { HttpErrorResponse } from '@angular/common/http';
import Swal from 'sweetalert2';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-create-promo-code',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-promo-code.component.html',
  styleUrl: './create-promo-code.component.css',
})
export class CreatePromoCodeComponent implements OnInit {
  promoForm!: FormGroup;
  serverErrors: { [key: string]: string[] } = {};
  eventId: number = 0;

  constructor(
    private fb: FormBuilder,
    private promoCodeService: PromoCodeService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.eventId = +params.get('eventId')!;

      this.promoForm = this.fb.group({
        promoCodeText: [
          '',
          [Validators.required, Validators.pattern(/^[a-zA-Z0-9]{8}$/)],
        ],
        saleAmountInPercentages: [
          0,
          [Validators.required, Validators.min(0), Validators.max(100)],
        ],
        promoCodeAmount: [0, [Validators.required, Validators.min(1)]],
      });
    });
  }

  onSubmit() {
    this.serverErrors = {};

    const dto: PromoCodeCreateDto = this.promoForm.value;
    dto.eventId = this.eventId;

    this.promoCodeService.createPromoCode(dto).subscribe({
      next: () => {
        Swal.fire('Success', 'Promo code created!', 'success');
        this.promoForm.reset();
      },
      error: (error: HttpErrorResponse) => {
        console.error(error);
        if (error.status === 400 && error.error.errors) {
          this.serverErrors = error.error.errors;
        } else {
          Swal.fire('Oops!', error.error.Message, 'error');
        }
      },
      complete: () => {
        this.router.navigate(['/events/event', this.eventId]);
      },
    });
  }
}
