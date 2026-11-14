using System;
using System.Collections.Generic;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.Domain.Model;
using TimerApplication.PresentationLayer.Helper;

namespace TimerApplication.PresentationLayer.View
{
    public class ConsoleOperator
    {
        private readonly UserService _userService;

        public ConsoleOperator(UserService userService)
        {
            this._userService = userService;
        }

        public void Run()
        {
            Console.WriteLine("Welcome to Time Logger");
            int choice;
            do
            {
                Console.WriteLine("1. Signup");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.WriteLine("3. Enter Your Option (1/2/3): ");
                bool isValidChoice = InputValidator.ValidateInteger(Console.ReadLine(), out choice);
                if (!isValidChoice)
                {
                    Console.WriteLine($"Invalid Integer format, Enter a valid integer!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        this.SignupUser();
                        break;
                    case 2:
                        this.LoginUser();
                        break;
                    case 3:
                        Console.WriteLine("Exiting the App, Bye!");
                        break;
                    default:
                        Console.WriteLine("Invalid Choice, Enter a valid choice between 1 to 3");
                        break;
                }
            }
            while (choice != 3);
        }

        private void LoginUser()
        {
            throw new NotImplementedException();
        }

        private void SignupUser()
        {
            var user = this.GetUserDetails();
            if (user == null)
            {
                return;
            }

            var isCreated = this._userService.CreateUser(user);
            if (!isCreated)
            {
                Console.WriteLine($"Signup failed, Account already registered with the given Email ID!");
                return;
            }

            Console.WriteLine("$Signup Successfull!");
        }

        private User GetUserDetails()
        {
            var userName = this.GetUserName();
            if (userName == null)
            {
                return null;
            }

            var email = this.GetEmail();
            if (email == null)
            {
                return null;
            }

            var hashedPassword = this.GetPassword();
            if (hashedPassword == null)
            {
                return null;
            }

            return new User(Guid.NewGuid(), userName, email, hashedPassword, new List<Activity>());
        }

        private string GetUserName()
        {
            Console.WriteLine("Enter User Name: ");
            var userName = Console.ReadLine();
            var isValidName = InputValidator.ValidateString(userName);
            if (!isValidName)
            {
                Console.WriteLine($"User Name should be null or empty!");
                return null;
            }

            return userName;
        }

        private string GetEmail()
        {
            Console.WriteLine("Enter User Email ID: ");
            var email = Console.ReadLine();
            var isValidEmail = InputValidator.ValidateEmail(email);
            if (!isValidEmail)
            {
                Console.WriteLine($"Invalid Email ID format");
                return null;
            }

            return email;
        }

        private string GetPassword()
        {
            Console.WriteLine("Enter Password: ");
            string password = "";
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }

                password += key.KeyChar;
                Console.Write("*");
            }

            Console.WriteLine();
            var isValidPassword = InputValidator.ValidatePassword(password);
            if (!isValidPassword.IsSuccess)
            {
                Console.WriteLine(isValidPassword.Message);
                return null;
            }

            var hashedPassword = PasswordHasher.HashPassword(password);
            return hashedPassword;
        }
    }
}
