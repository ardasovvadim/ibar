import {Component, OnInit} from '@angular/core';
import {LoadingService} from '../../../core/services/loading.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.sass']
})
export class SettingsComponent implements OnInit {

  constructor(private loading: LoadingService) {
  }

  ngOnInit() {

  }

}
