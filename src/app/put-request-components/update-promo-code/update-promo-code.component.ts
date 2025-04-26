import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  PromoCodeService,
  PromoCodeUpdateDto,
} from '../../services/promo-code/promo-code.service';
import { ActivatedRoute, Router } from '@angular/router';
import Swal from 'sweetalert2';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-update-promo-code',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './update-promo-code.component.html',
  styleUrl: './update-promo-code.component.css',
})
export class UpdatePromoCodeComponent implements OnInit {
  promoForm!: FormGroup;
  serverErrors: { [key: string]: string[] } = {};
  promoId: number = 0;
  organizerId: number = 0;
  promoAmont: number = 0;

  constructor(
    private fb: FormBuilder,
    private promoCodeService: PromoCodeService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.promoId = +params.get('promoCodeId')!;
      this.organizerId = +params.get('organizerId')!;
      this.getPromoCode();

      this.promoForm = this.fb.group({
        promoCodeAmount: [0, [Validators.required, Validators.min(1)]],
      });
    });
  }

  getPromoCode() {
    this.promoCodeService.getPromoCodeById(this.promoId).subscribe({
      next: (data: any) => {
        this.promoAmont = data.promoCode.promoCodeAmount;
        this.promoForm.patchValue({
          promoCodeAmount: this.promoAmont,
        });
      },
      error: (err) => {
        Swal.fire('Oops!', err.error.Message, 'error');
      },
    });
  }

  onSubmit() {
    this.serverErrors = {};

    const dto: PromoCodeUpdateDto = this.promoForm.value;

    this.promoCodeService.updatePromoCode(this.promoId, dto).subscribe({
      next: () => {
        Swal.fire('Success!', 'Promo code updated successfully!', 'success');
        this.promoForm.reset();
      },
      error: (error: HttpErrorResponse) => {
        if (error.status === 400 && error.error.errors) {
          this.serverErrors = error.error.errors;
        } else {
          Swal.fire('Oops!', 'An unexpected error occurred.', 'error');
        }
      },
      complete: () => {
        this.router.navigate(['/organizer-panel', this.organizerId]);
      },
    });
  }
}
