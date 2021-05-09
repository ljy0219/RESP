import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrisContactComponent } from './hris-contact.component';

describe('HrisContactComponent', () => {
  let component: HrisContactComponent;
  let fixture: ComponentFixture<HrisContactComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrisContactComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrisContactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
