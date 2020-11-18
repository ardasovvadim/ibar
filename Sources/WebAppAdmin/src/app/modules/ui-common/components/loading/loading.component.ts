import { Component, OnInit } from '@angular/core';
import {LoadingService} from '../../../core/services/loading.service';

@Component({
  selector: 'app-loading',
  templateUrl: './loading.component.html',
  styleUrls: ['./loading.component.sass']
})
export class LoadingComponent implements OnInit {

  isLoading = false;

  constructor(private loading: LoadingService) { }

  ngOnInit() {
    this.loading.isLoading.subscribe(value => this.isLoading = value);
  }

}
