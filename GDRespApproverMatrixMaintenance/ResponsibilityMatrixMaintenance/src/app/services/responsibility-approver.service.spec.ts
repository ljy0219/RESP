import { TestBed, inject } from '@angular/core/testing';

import { ResponsibilityApproverService } from './responsibility-approver.service';

describe('ResponsibilityApproverService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ResponsibilityApproverService]
    });
  });

  it('should be created', inject([ResponsibilityApproverService], (service: ResponsibilityApproverService) => {
    expect(service).toBeTruthy();
  }));
});
