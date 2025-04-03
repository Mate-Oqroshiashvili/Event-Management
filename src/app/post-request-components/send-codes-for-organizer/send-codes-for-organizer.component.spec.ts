import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SendCodesForOrganizerComponent } from './send-codes-for-organizer.component';

describe('SendCodesForOrganizerComponent', () => {
  let component: SendCodesForOrganizerComponent;
  let fixture: ComponentFixture<SendCodesForOrganizerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SendCodesForOrganizerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SendCodesForOrganizerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
