import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddSpeakerOnEventComponent } from './add-speaker-on-event.component';

describe('AddSpeakerOnEventComponent', () => {
  let component: AddSpeakerOnEventComponent;
  let fixture: ComponentFixture<AddSpeakerOnEventComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddSpeakerOnEventComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddSpeakerOnEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
