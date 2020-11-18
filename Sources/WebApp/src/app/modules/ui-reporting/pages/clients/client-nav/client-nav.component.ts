import {Component, OnInit} from '@angular/core';
import {ClientsService} from '../clients.service';
import {TradeAccountModel} from '../../../../core/models/trade-account.model';

@Component({
  selector: 'app-client-nav',
  templateUrl: './client-nav.component.html',
  styleUrls: ['./client-nav.component.sass']
})
export class ClientNavComponent implements OnInit {

  accountHeader: {id: string; name: string} = null;

  constructor(private clientsService: ClientsService) {
  }

  ngOnInit() {
    this.clientsService.accountHeader.subscribe(data => {
      if (data) {
        this.accountHeader = data;
      }
    });
  }

}
