import {Injectable} from '@angular/core';
import {environment} from '../../../../environments/environment';
import {Subject} from 'rxjs';

declare var $: any;

@Injectable()
export class SignalRService {

  private baseUrl = environment.signalrUrl;
  private connection;

  connectionId: string;


  private responseSubject = new Subject<string>();
  response = this.responseSubject.asObservable();

  constructor() {
    this.init();
  }

  init() {
    this.buildConnection();
    this.registerSignalEvents();
  }

  get connectionIdValue(): string {
    return this.connectionId;
  }

  private buildConnection() {
    this.responseSubject.next('Connecting...');
    this.connection = $.connection(this.baseUrl);
  }

  private registerSignalEvents() {
    this.connection.received((data) => {
      this.responseSubject.next(data);
    });
  }

  send(msg: string) {
    this.connection.start().done(() => {
      this.connectionId = this.connection.id;
      this.connection.send(msg);
    });
  }

}
