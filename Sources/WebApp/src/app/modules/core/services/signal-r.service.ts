import {Injectable} from '@angular/core';
import {environment} from '../../../../environments/environment';
import {Subject} from 'rxjs';

declare var $: any;

@Injectable()
export class SignalRService {

  private baseUrl = environment.signalrUrl;
  private _connection;

  // private connectionIdSubject = new BehaviorSubject<string>(null);
  // connectionId = this.connectionIdSubject.asObservable();
  connectionId: string;


  private requestsSubject = new Subject<string>();
  request = this.requestsSubject.asObservable();

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
    this._connection = $.connection(this.baseUrl);
  }

  private registerSignalEvents() {
    this._connection.received((data) => {
      const that = this;
      setTimeout(() => that.requestsSubject.next(data), 10);
    });
  }

  send(msg: string) {
    this._connection.start().done(() => {
      this.connectionId = this._connection.id;
      this._connection.send(msg);
    });
  }

}
