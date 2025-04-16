import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddArtistOnEventComponent } from './add-artist-on-event.component';

describe('AddArtistOnEventComponent', () => {
  let component: AddArtistOnEventComponent;
  let fixture: ComponentFixture<AddArtistOnEventComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddArtistOnEventComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddArtistOnEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
