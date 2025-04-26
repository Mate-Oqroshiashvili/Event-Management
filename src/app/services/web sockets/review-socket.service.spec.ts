import { TestBed } from '@angular/core/testing';

import { ReviewSocketService } from './review-socket.service';

describe('ReviewSocketService', () => {
  let service: ReviewSocketService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReviewSocketService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
