import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdatePromoCodeComponent } from './update-promo-code.component';

describe('UpdatePromoCodeComponent', () => {
  let component: UpdatePromoCodeComponent;
  let fixture: ComponentFixture<UpdatePromoCodeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdatePromoCodeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdatePromoCodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
