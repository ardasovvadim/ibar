import {AccountModel} from '../modules/ui-reporting/pages/clients/models/account.model';
import {TRADING_PERMISSION_1} from './trading-permissin-1';

export const ACCOUNT_1: AccountModel = {
  id: 0,
  name: 'Arthur Brady',
  country: 'Ukraine',
  city: 'Kharkiv',
  postcode: 61000,
  street: '62 S. Valley Farms Drive Amarillo, TX 79106',
  email: 'arthurbrady@gmail.com',
  accountNumber: 101,
  accountType: 'Basic',
  dateOpened: new Date(2020, 1, 1),
  currency: 'USD',
  customerType: 'Basic',
  dateFunded: new Date(),
  cash: 1500,
  master: 'Master account',
  dateClosed: null,
  tradingPermissions: TRADING_PERMISSION_1,
  notes: [
    'Too cultivated use solicitude frequently. Dashwood likewise up consider continue entrance ' +
    'ladyship oh. Wrong guest given purse power is no.',
    'Please check your account'
  ]
};
