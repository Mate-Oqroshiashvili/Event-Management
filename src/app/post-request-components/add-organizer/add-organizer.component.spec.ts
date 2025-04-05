import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddOrganizerComponent } from './add-organizer.component';

describe('AddOrganizerComponent', () => {
  let component: AddOrganizerComponent;
  let fixture: ComponentFixture<AddOrganizerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddOrganizerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddOrganizerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
