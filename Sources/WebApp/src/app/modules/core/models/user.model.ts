import {IEntityModel} from './i-entity.model';

export class UserModel implements IEntityModel {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phone: string;
  verificationCode: number;
  accessToken: string;
}
