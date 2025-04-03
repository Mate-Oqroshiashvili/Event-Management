import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatePromoCodeComponent } from './create-promo-code.component';

describe('CreatePromoCodeComponent', () => {
  let component: CreatePromoCodeComponent;
  let fixture: ComponentFixture<CreatePromoCodeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreatePromoCodeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreatePromoCodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
