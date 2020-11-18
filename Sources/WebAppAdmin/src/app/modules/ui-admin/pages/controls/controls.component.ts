import {Component, OnInit} from '@angular/core';
import {ControlService} from './control.service';
import {SignalRService} from '../../../core/services/signal-r.service';
import {JobEnum} from './job-enum.enum';

@Component({
  selector: 'app-controls',
  templateUrl: './controls.component.html',
  styleUrls: ['./controls.component.sass'],
  providers: [ControlService, SignalRService]
})
export class ControlsComponent implements OnInit {

  jobEnum = JobEnum;

  constructor() {
  }

  ngOnInit() {
  }

}
