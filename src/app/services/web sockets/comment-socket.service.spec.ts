import { TestBed } from '@angular/core/testing';

import { CommentSocketService } from './comment-socket.service';

describe('CommentSocketService', () => {
  let service: CommentSocketService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommentSocketService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
