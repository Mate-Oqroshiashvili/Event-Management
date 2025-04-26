import { TestBed } from '@angular/core/testing';

import { ParticipantSocketService } from './participant-socket.service';

describe('ParticipantSocketService', () => {
  let service: ParticipantSocketService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ParticipantSocketService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
