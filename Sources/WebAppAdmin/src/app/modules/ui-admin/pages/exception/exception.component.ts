import {Component, OnInit} from '@angular/core';
import {ApiService} from '../../../core/services/api.service';
import {ActivatedRoute, Params} from '@angular/router';
import {ExceptionService} from './exception.service';

@Component({
  selector: 'app-exception',
  templateUrl: './exception.component.html',
  styleUrls: ['./exception.component.sass'],
  providers: []
})
export class ExceptionComponent implements OnInit {

  id: number;
  exception: string;

  constructor(private route: ActivatedRoute,
              private exceptService: ExceptionService,
              private api: ApiService) {
  }

  ngOnInit() {
    this.route.params
      .subscribe(
        (params: Params) => {
          this.id = +params['id'];
          this.exceptService.getException(this.id.toString()).subscribe(data =>
            this.exception = data);
        }
      );
  }

}
