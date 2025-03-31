import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvailablePromoCodesComponent } from './available-promo-codes.component';

describe('AvailablePromoCodesComponent', () => {
  let component: AvailablePromoCodesComponent;
  let fixture: ComponentFixture<AvailablePromoCodesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvailablePromoCodesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AvailablePromoCodesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
