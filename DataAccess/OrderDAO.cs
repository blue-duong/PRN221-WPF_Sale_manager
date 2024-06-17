using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess
{
    public class OrderDAO
    {
        private static OrderDAO instance = null;
        private static readonly object instanceLock = new object();

        private OrderDAO() { }

        public static OrderDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Order> List()
        {
            try
            {
                using (var context = new SaleManagerContext())
                {
                    return context.Orders.OrderByDescending(order => order.OrderId).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Order FindOne(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                using (var context = new SaleManagerContext())
                {
                    return context.Orders.SingleOrDefault(predicate);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public Order FindById(int orderId)
        {
            using (var context = new SaleManagerContext())
            {
                return context.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.OrderId == orderId);
            }
        }

        public IEnumerable<Order> FindAll(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                using (var context = new SaleManagerContext())
                {
                    return context.Orders.Where(predicate).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                using (var context = new SaleManagerContext())
                {
                    return context.OrderDetails.Where(od => od.OrderId == orderId).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public void Add(Order order)
        {
            try
            {
                using (var context = new SaleManagerContext())
                {
                    context.Orders.Add(order);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Delete(Order order)
        {
            try
            {
                using (var context = new SaleManagerContext())
                {
                    context.Orders.Remove(order);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(Order order)
        {
            try
            {
                using (var context = new SaleManagerContext())
                {
                    // Đính kèm thực thể Order vào ngữ cảnh
                    context.Orders.Attach(order);

                    // Đánh dấu thực thể đã được sửa đổi
                    context.Entry(order).State = EntityState.Modified;

                    // Đánh dấu các chi tiết đơn hàng là đã thêm mới
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        context.Entry(orderDetail).State = EntityState.Added;
                    }

                    // Save changes with optimistic concurrency handling
                    bool saveFailed;
                    do
                    {
                        saveFailed = false;
                        try
                        {
                            context.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;

                            // Get the current entity values and the values in the database
                            var entry = ex.Entries.Single();
                            var clientValues = entry.CurrentValues;
                            var databaseEntry = entry.GetDatabaseValues();

                            if (databaseEntry == null)
                            {
                                throw new Exception("The order was deleted by another user.");
                            }

                            var databaseValues = databaseEntry.ToObject();

                            // Refresh the original values to bypass the concurrency issue
                            entry.OriginalValues.SetValues(databaseEntry);
                        }
                    } while (saveFailed);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void RemoveOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                using (var context = new SaleManagerContext())
                {
                    context.OrderDetails.Remove(orderDetail);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public decimal GetOrderTotal(int orderId)
        {
            using (var context = new SaleManagerContext())
            {
                return context.OrderDetails
                    .Where(od => od.OrderId == orderId)
                    .Sum(od => od.UnitPrice * od.Quantity);
            }
        }

    }
}
