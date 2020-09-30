import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-generatepdf',
  templateUrl: './generatepdf.component.html',
  styleUrls: ['./generatepdf.component.css']
})
export class GeneratepdfComponent implements OnInit {

  httpclient: HttpClient;
  baseurl: string = "";
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpclient = http;
    this.baseurl = baseUrl;
  }

  ngOnInit() {
  }

  generateform() {
    this.httpclient.get('/Certificate/GeneratePDF').subscribe(result => {
      var rtn = result;
      alert("Successfully Generateted!!!");
    }, error => console.error(error));
  }
}
