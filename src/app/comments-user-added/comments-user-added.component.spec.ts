import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommentsUserAddedComponent } from './comments-user-added.component';

describe('CommentsUserAddedComponent', () => {
  let component: CommentsUserAddedComponent;
  let fixture: ComponentFixture<CommentsUserAddedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommentsUserAddedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommentsUserAddedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
