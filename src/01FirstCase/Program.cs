/*
 案例：影片出租店，顾客消费金额并打印详情单，操作者使用程序登记影片和时长，影片由分类，普通、儿童和新片。常客计算积分(VIP)，积分会根据种类是否为新片而有不同。
 */
namespace _01FirstCase
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    // 影片类
    public class Movie
    {
        public const int CHILDRENS = 2;
        public const int REGULAR = 0;
        public const int NEW_RSLEASE = 1;

        private string _title;
        private int _priceCode;

        public Movie(string title, int priceCode)
        {
            _title = title;
            _priceCode = priceCode;
        }

        public int PriceCode { get { return _priceCode; } set { _priceCode = value; } }

        public string Title { get { return _title; } }

        // 重构 AmountFor 更名 GetCharge
        // 重构 此方法第二步 将移到Movie类中， 新增参数 daysRented
        public double GetCharge(int daysRented)
        {
            double result = 0;
            switch (PriceCode)
            {
                case Movie.REGULAR:
                    result += 2;
                    if (daysRented > 2)
                        result += (daysRented - 2) * 1.5;
                    break;
                case Movie.NEW_RSLEASE:
                    result += daysRented * 3;
                    break;
                case Movie.CHILDRENS:
                    result += 1.5;
                    if (daysRented > 3)
                        result += (daysRented - 3) * 1.5;
                    break;
            }
            return result;
        }

        public int GetFrequentRenterPoints(int daysRented)
        {
            if ((PriceCode == Movie.NEW_RSLEASE) && daysRented > 1)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }

    // 租赁类
    public class Rental
    {
        private Movie _movie;
        private int _daysRented;

        public Rental(Movie movie, int daysRented) { _movie = movie; _daysRented = daysRented; }

        public Movie Movie { get { return _movie; } }

        public int DaysRented { get { return _daysRented; } }

        // 重构 第二步 移动到Movie
        public int GetFrequentReterPoints()
        {
            return _movie.GetFrequentRenterPoints(_daysRented);
        }

        public double GetCharge()
        {
            return _movie.GetCharge(_daysRented);
        }
    }

    // 顾客类
    public class Customer
    {
        private string _name;
        private List<Rental> _rentals = new List<Rental>();

        public Customer(string name)
        {
            _name = name;
        }

        public void AddRental(Rental arg)
        {
            _rentals.Add(arg);
        }

        public string Name { get { return _name; } }

        // 用于生成详单的函数
        // Statement 声明；陈述；报表；报告
        public string Statement_old()
        {
            double totalAmount = 0;
            int frequentReterPoints = 0;
            string result = $"Rental Record for {Name} \n";
            foreach (var rental in _rentals)
            {
                double thisAmount = 0;
                // 此处重构
                switch (rental.Movie.PriceCode)
                {
                    case Movie.REGULAR:
                        thisAmount += 2;
                        if (rental.DaysRented > 2)
                            thisAmount += (rental.DaysRented - 2) * 1.5;
                        break;
                    case Movie.NEW_RSLEASE:
                        thisAmount += rental.DaysRented * 3;
                        break;
                    case Movie.CHILDRENS:
                        thisAmount += 1.5;
                        if (rental.DaysRented > 3)
                            thisAmount += (rental.DaysRented - 3) * 1.5;
                        break;
                }
                // 此处结束

                frequentReterPoints++;

                if (rental.Movie.PriceCode == Movie.NEW_RSLEASE && rental.DaysRented > 1)
                {
                    frequentReterPoints++;
                }

                result += "\t" + rental.Movie.Title + "\t" + thisAmount.ToString() + "\n";
                totalAmount += thisAmount;
            }

            result += "Amount owed is " + totalAmount.ToString() + "\n";
            result += "You earned " + frequentReterPoints.ToString() + " frequent renter points";
            return result;
        }

        public string Statement_new()
        {
            //double totalAmount = 0;  // 重构 Replace Temp with Query，并利用查询函数(query method) 来代替临时变量
            //int frequentReterPoints = 0; // 重构 Replace Temp with Query
            string result = $"Rental Record for {Name} \n";
            foreach (var rental in _rentals)
            {
                //double thisAmount = 0;
                // 此处重构
                //thisAmount = AmountFor(rental); ==>
                // thisAmount = rental.GetCharge(); 多余没修改变量
                // 此处结束

                //frequentReterPoints++;
                //if (rental.Movie.PriceCode == Movie.NEW_RSLEASE && rental.DaysRented > 1)
                //{
                //    frequentReterPoints++;
                //} ==> Extract Method(重构 提取方法)

                //frequentReterPoints += rental.FrequentReterPoints();

                result += "\t" + rental.Movie.Title + "\t" + rental.GetCharge().ToString() + "\n";
                //totalAmount += rental.GetCharge();
            }

            //result += "Amount owed is " + totalAmount.ToString() + "\n";
            //result += "You earned " + frequentReterPoints.ToString() + " frequent renter points";
            result += "Amount owed is " + GetTotalCharge().ToString() + "\n";
            result += "You earned " + GetTotalFrequentRenterPoints().ToString() + " frequent renter points";
            return result;
        }

        private double GetTotalCharge()
        {
            double result = 0;
            foreach (var rental in _rentals)
            {
                result += rental.GetCharge();
            }
            return result;
        }

        private double GetTotalFrequentRenterPoints()
        {
            double result = 0;
            foreach (var rental in _rentals)
            {
                result += rental.GetFrequentReterPoints();
            }
            return result;
        }

        // 重构从 Customer类 移动到 Rental类
        //private double AmountFor(Rental rental)
        //{
        //    double result = 0;
        //    switch (rental.Movie.PriceCode)
        //    {
        //        case Movie.REGULAR:
        //            result += 2;
        //            if (rental.DaysRented > 2)
        //                result += (rental.DaysRented - 2) * 1.5;
        //            break;
        //        case Movie.NEW_RSLEASE:
        //            result += rental.DaysRented * 3;
        //            break;
        //        case Movie.CHILDRENS:
        //            result += 1.5;
        //            if (rental.DaysRented > 3)
        //                result += (rental.DaysRented - 3) * 1.5;
        //            break;
        //    }
        //    return result;
        //}

        // 重构代码内容
        private double AmountFor(Rental rental)
        {
            return rental.GetCharge();
        }

    }

    // 建立Movie的三个子类
    public class Regular_Movie
    {

    }

    public class Childrens_Movie
    {

    }

    public class New_Release_Movie
    {

    }

}
