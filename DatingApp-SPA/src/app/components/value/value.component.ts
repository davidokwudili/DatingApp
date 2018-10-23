import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {

  values: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    // call after the comp has been loaded
    this.getValues();
  }

  getValues() {
    this.http.get('http://localhost:5000/api/values').subscribe(_request => {
      this.values = _request;
    }, _error => {
      console.log(_error);
    });
  }


}
