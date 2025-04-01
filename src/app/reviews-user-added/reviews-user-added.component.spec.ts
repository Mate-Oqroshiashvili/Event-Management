import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewsUserAddedComponent } from './reviews-user-added.component';

describe('ReviewsUserAddedComponent', () => {
  let component: ReviewsUserAddedComponent;
  let fixture: ComponentFixture<ReviewsUserAddedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReviewsUserAddedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReviewsUserAddedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
