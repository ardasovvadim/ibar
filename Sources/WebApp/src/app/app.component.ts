import {Component, OnInit} from '@angular/core';
import {AuthenticationService} from './modules/core/services/authentication.service';
import {LoadingService} from './modules/core/services/loading.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent implements OnInit {

  isCurrentUser: boolean;

  constructor(private authenticationService: AuthenticationService,
              private loadingService: LoadingService) {
  }

  ngOnInit(): void {
    this.loadingService.create();
    this.authenticationService.currentUser.subscribe(value => this.isCurrentUser = !!value);
  }

}
