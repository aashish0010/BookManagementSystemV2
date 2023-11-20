import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-defaultpage',
  templateUrl: './defaultpage.component.html',
  styleUrls: ['./defaultpage.component.scss']
})
export class DefaultpageComponent implements OnInit {
  isLoggedIn$ : boolean | undefined;
  ngOnInit(): void {
    this.isLoggedIn$ = false;
  }
}

