export class UserModel {
  id: number;
  firstName: string;
  lastName: string;
  phone: string;
  password: string;
  email: string;
  roles: string[];
  isWaitingConfirmation: boolean;
}
