import { UserModel } from '../modules/core/models/user.model';
import { ChartDataModel } from '../modules/core/models/chart-data.model';
import { MasterAccountModel } from '../modules/core/models/master-account.model';

export const DATA: ChartDataModel[] = [
  {
    id: 0,
    total: [],
    data: [
      { data: [65, 59, 80, 81, 56, 55, 40], label: 'Account A' },
      { data: [28, 48, 40, 19, 86, 27, 90], label: 'Account B' },
      { data: [13, 20, 54, 36, 76, 55, 88], label: 'Account C' },
      { data: [52, 73, 28, 67, 37, 95, 23], label: 'Account D' }
    ],
    labels: ['2006', '2007', '2008', '2009', '2010', '2011', '2012'],
  },
  {
    id: 0,
    total: [],
    data: [
      { data: [65, 59, 80, 81, 56, 55, 40], label: 'Series A' },
    ],
    labels: ['2006', '2007', '2008', '2009', '2010', '2011', '2012']
  },
  {
    id: 0,
    total: [],
    data: [
      { data: [63367, 12425, 23414, 35423], label: 'Account A' },
      { data: [45235, 12412, 13412, 345234], label: 'Account B' },
      { data: [34251, 21425, 12366, 135453], label: 'Account C' },
    ],
    labels: ['2006', '2007', '2008', '2009']
  },
  {
    id: 0,
    total: [],
    data: [
      { data: [142, 67, 235, 132, 256, 200], label: 'Series A' },
    ],
    labels: ['2006', '2007', '2008', '2009', '2010', '2011']
  },
  {
    id: 0,
    total: [],
    data: [
      { data: [636, 135, 123, 543, 123, 123, 634, 631], label: 'Inbound' },
      { data: [734, 123, 782, 123, 674, 213, 473, 736], label: 'Outbound' },
    ],
    labels: ['', '', '', '', '', '', '', '']
  },
  {
    id: 0,
    total: [],
    data: [
      { data: [200, 500], label: null },
    ],
    labels: ['Inbound', 'Outbound']
  },
  {
    id: 7,
    total: [],
    data: [
      { data: [100, 200, 300], label: null },
    ],
    labels: ['A', 'B', 'C']
  },
  {
    id: 8,
    total: [],
    data: [
      { data: [100, 200, 300, 400, 500], label: null },
    ],
    labels: ['Stocks', 'Cash', 'Options', 'Futures', 'Other']
  },
  {
    id: 9,
    total: [],
    data: [
      { data: [100, 200, 300, 400], label: null },
    ],
    labels: ['USD', 'EUR', 'ILS', 'Other']
  },
];

export const TOTALDATA = [
  21324,
  64825,
  73732,
  63731,
];

export const MASTER_ACCOUNTS = [
  {
    id: 1,
    accountName: 'Private accounts'
  },
  {
    id: 2,
    accountName: 'Public accounts'
  }
];

export const USER: any = { id: 1, name: 'Joel Bryant' };
