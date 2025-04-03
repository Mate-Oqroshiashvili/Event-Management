import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddOrganizerOnLocationComponent } from './add-organizer-on-location.component';

describe('AddOrganizerOnLocationComponent', () => {
  let component: AddOrganizerOnLocationComponent;
  let fixture: ComponentFixture<AddOrganizerOnLocationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddOrganizerOnLocationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddOrganizerOnLocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
