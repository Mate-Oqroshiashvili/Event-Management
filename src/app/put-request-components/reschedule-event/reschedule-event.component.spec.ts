import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RescheduleEventComponent } from './reschedule-event.component';

describe('RescheduleEventComponent', () => {
  let component: RescheduleEventComponent;
  let fixture: ComponentFixture<RescheduleEventComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RescheduleEventComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RescheduleEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
