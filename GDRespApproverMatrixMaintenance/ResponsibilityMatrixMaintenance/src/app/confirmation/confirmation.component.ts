import { Component, OnInit ,Input, Output,EventEmitter} from '@angular/core';

@Component({
  selector: 'app-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.css']
})
export class ConfirmationComponent implements OnInit {

  @Output() confirm =new EventEmitter();
  constructor() { }

  ngOnInit() {
  }

  onYes()
  {
    this.confirm.emit(true);
  }

  onNo()
  {
    this.confirm.emit(false);
  }

}
