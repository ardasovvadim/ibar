import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ClientsTradesService} from '../clients-trades.service';
import {AccountTradesDetailsModel} from '../client-trade-details.model';
import {catchError} from 'rxjs/operators';
import {of} from 'rxjs';
import {TRADES} from '../../../../../../mock-data/trades';
import {Period} from '../../../../../core/models/period';

@Component({
  selector: 'app-trade-block',
  templateUrl: './trade-block.component.html',
  styleUrls: ['./trade-block.component.sass']
})

export class TradeBlockComponent implements OnInit {
  @Input() dataDetails: AccountTradesDetailsModel = new AccountTradesDetailsModel();
  @Input() index = 0;
  constructor() { }

  ngOnInit() {

  }

}
