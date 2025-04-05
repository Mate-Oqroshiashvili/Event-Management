import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizerPanelComponent } from './organizer-panel.component';

describe('OrganizerPanelComponent', () => {
  let component: OrganizerPanelComponent;
  let fixture: ComponentFixture<OrganizerPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrganizerPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrganizerPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
