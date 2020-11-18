import {Component, OnInit} from '@angular/core';
import {AuthenticationService} from './modules/core/services/authentication.service';
import {LoadingService} from './modules/core/services/loading.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent implements OnInit {
  title = 'WebAppAdmin';
  isCurrentAdminUser: boolean;

  constructor(private authService: AuthenticationService) {
  }

  ngOnInit(): void {
    this.authService.currentAdminUser.subscribe(user => this.isCurrentAdminUser = !!user);
  }
}
