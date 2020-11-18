import {ChangeDetectorRef, Component, Input, OnInit} from '@angular/core';
import {JobStatus} from '../job-status.enum';
import {SignalRService} from '../../../../core/services/signal-r.service';
import {ControlService} from '../control.service';
import {catchError} from 'rxjs/operators';
import {throwError} from 'rxjs';
import {JobEnum} from '../job-enum.enum';

@Component({
  selector: 'app-job-control',
  templateUrl: './job-control.component.html',
  styleUrls: ['./job-control.component.sass']
})
export class JobControlComponent implements OnInit {

  status: JobStatus = null;
  @Input() job: JobEnum;
  private jobName;
  logs: ResponseMessage[] = [];


  constructor(private signalR: SignalRService,
              private cdr: ChangeDetectorRef,
              private controlService: ControlService) {
  }

  ngOnInit() {
    this.jobName = JobEnum[this.job];

    this.signalR.send(JobEnum[this.job]);

    this.signalR.response.subscribe(data => this.ParseResponse(data));

    this.controlService.getStatusJob(this.jobName).subscribe();
  }

  private ParseResponse(data: string) {
      if (data.includes('status')) {
        const status = data.split('$')[1].toUpperCase();
        switch (JobStatus[status]) {
          case JobStatus.RUNNING:
            this.status = JobStatus.RUNNING;
            break;
          case JobStatus.STOPPED:
            this.status = JobStatus.STOPPED;
            break;
          case JobStatus.INITIALIZING:
            this.status = JobStatus.INITIALIZING;
            break;
          case JobStatus.ERROR:
            this.status = JobStatus.ERROR;
            break;
        }
        return;
      }
      const resp = new ResponseMessage();
      resp.message = data;
      this.logs.push(resp);
      this.cdr.detectChanges();
  }


  startJob() {
    this.controlService.startJob(this.jobName).pipe(
      catchError(err => {
        return throwError(err);
      })
    ).subscribe(() => {
      this.status = JobStatus.RUNNING;
    });
  }

  stopJob() {
    this.controlService.stopJob(this.jobName).pipe(
      catchError(err => {
        return throwError(err);
      })
    ).subscribe(() => {
      this.status = JobStatus.STOPPED;
    });
  }

  reloadJob() {
    this.controlService.reloadJob(this.jobName).pipe(
      catchError(err => {
        return throwError(err);
      })
    ).subscribe(() => {
      this.status = JobStatus.RUNNING;
    });
  }

  getStatusString(): string {
    switch (this.status) {
      case JobStatus.RUNNING:
        return 'Running';
      case JobStatus.STOPPED:
        return 'Stopped';
      case JobStatus.ERROR:
        return 'Error';
      case JobStatus.INITIALIZING:
        return 'Initializing';
      default:
        return '';
    }
  }

  isDisabledStopButton(): boolean {
    return this.status === JobStatus.STOPPED || this.status === JobStatus.ERROR;
  }

  isDisabledStartButton(): boolean {
    return this.status === JobStatus.RUNNING;
  }

  isDisabledReloadButton() {
    return !(this.status === JobStatus.RUNNING);
  }
}
export  class  ResponseMessage {
  id?: number;
  message: string;
  link?: string;
}
