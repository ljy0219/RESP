import { TestBed, inject } from '@angular/core/testing';

import { HrisContactService } from './hris-contact.service';

describe('HrisContactService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HrisContactService]
    });
  });

  it('should be created', inject([HrisContactService], (service: HrisContactService) => {
    expect(service).toBeTruthy();
  }));
});
