import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserParticipationHistoryComponent } from './user-participation-history.component';

describe('UserParticipationHistoryComponent', () => {
  let component: UserParticipationHistoryComponent;
  let fixture: ComponentFixture<UserParticipationHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserParticipationHistoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserParticipationHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
