import { Component, OnInit } from '@angular/core';
import { Config } from '../config';
import { ResponseObject } from '../models/responseObject';

@Component({
    selector: 'app-noaccess',
    templateUrl: './noaccess.component.html',
    styleUrls: ['./noaccess.component.css']
})
export class NoaccessComponent implements OnInit {

  constructor( private config: Config) { }

  ngOnInit() {

  }

}
