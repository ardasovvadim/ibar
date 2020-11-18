import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {TradeModel} from '../../models/trade.model';
import {PortfolioVm} from '../../models/portfolioVm';
import {ClientsService} from '../../clients.service';

@Component({
  selector: 'app-porfolio-block',
  templateUrl: './porfolio-block.component.html',
  styleUrls: ['./porfolio-block.component.sass']
})
export class PorfolioBlockComponent implements OnInit {

  @Input() idClient: number;
  @Input() portfolioDate: Date = new Date();
  @Input() isNext = false;
  @Output() nextEvent = new EventEmitter();
  portfolioData: PortfolioVm;

  constructor(private clientsService: ClientsService) { }

  ngOnInit() {
    this.clientsService.getPortfolioData('').subscribe(data => this.portfolioData = data);
  }

  nextPortfolio(): void {
    this.nextEvent.emit();
  }

}
