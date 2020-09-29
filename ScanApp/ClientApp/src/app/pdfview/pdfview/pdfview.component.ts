import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-pdfview',
  templateUrl: './pdfview.component.html',
  styleUrls: ['./pdfview.component.css']
})
export class PdfviewComponent implements OnInit {

  src = "/assets/5678.pdf";
  constructor() { }

  ngOnInit() {
  }

}
